using _2048ConsoleEdition.Gameplay;
using _2048ConsoleEdition.Graphics;
using _2048ConsoleEdition.Input;
using _2048ConsoleEdition.Saves;

namespace _2048ConsoleEdition
{
    public class Game : IScoreProvider
    {
        private readonly IDrawer _drawer;
        private readonly IInput _input;
        private readonly ISaveProvider _saveProvider;
        private readonly Field _field;
        
        private GameState _state;

        public int Score { get; private set; }
        public int BestScore { get; private set; }

        public bool IsPlaying => _state != GameState.QuitAccepted;
        
        public Game(IDrawer drawer, IInput input, ISaveProvider saveProvider)
        {
            _drawer = drawer;
            _input = input;
            _saveProvider = saveProvider;
            
            _field = new Field(Configuration.RowCount, Configuration.ColumnCount);
            _state = GameState.Normal;
            
            Score = 0;
            
            SubscribeToInputEvents();
        }

        private void SubscribeToInputEvents()
        {
            _input.OnMoveRequested += Move;
            _input.OnQuitRequested += RequestedQuit;
            _input.OnRestartRequested += RequestRestart;
            _input.OnConfirmed += ConfirmRequest;
            _input.OnCanceled += CancelRequest;
            _input.OnForcedQuit += Quit;
        }  
        
        private void UnsubscribeFromInputEvents()
        {
            _input.OnMoveRequested -= Move;
            _input.OnQuitRequested -= RequestedQuit;
            _input.OnRestartRequested -= RequestRestart;
            _input.OnConfirmed -= ConfirmRequest;
            _input.OnCanceled -= CancelRequest;
            _input.OnForcedQuit -= Quit;
        }

        public async Task RunAsync()
        {
            _drawer.DrawLoader();
            
            await _saveProvider.InitializeAsync();
            
            BestScore = _saveProvider.BestScore;

            await Start();
        }

        private async Task Start()
        {
            _field.PutNewValue();
            _field.PutNewValue();
            
            await Update();
        }
        
        private async Task Update()
        {
            while (_state is not GameState.QuitAccepted and not GameState.RestartAccepted)
            {
                _field.Update();

                _drawer.Draw(this, _state, _field);
                _input.WaitForUserInput();
            }

            UnsubscribeFromInputEvents();

            await Save();
        }
        
        private void RequestedQuit()
        {
            _state = GameState.QuitRequested;
        }

        private void RequestRestart()
        {
            _state = GameState.RestartRequested;
        }

        private void ConfirmRequest()
        {
            _state = _state switch
            {
                GameState.RestartRequested => GameState.RestartAccepted,
                GameState.QuitRequested => GameState.QuitAccepted,
                _ => _state
            };
        }

        private void CancelRequest()
        {
            _state = GameState.Normal;
        }

        private void Move(Direction dir)
        {
            _field.Move(dir, out var score);
            
            Score += score;
            if (Score > BestScore)
            {
                BestScore = Score;
            }
        }

        private async Task Save()
        {
            _saveProvider.BestScore = Score;
            await _saveProvider.SaveAsync();
        }

        private void Quit()
        {
            Save();
        }
    }
}