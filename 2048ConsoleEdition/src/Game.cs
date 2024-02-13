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
        
        private Field _field;
        private GameState _state = GameState.Normal;

        public int Score { get; private set; }
        public int BestScore { get; private set; }
        
        public Game()
        {
            _drawer = new ConsoleDrawer();
            _input = new ConsoleInput();
            _saveProvider = new SaveProvider();
            
            SubscribeToInputEvents();

            Reset();
        }

        private void SubscribeToInputEvents()
        {
            _input.OnMoveRequested += Move;
            _input.OnQuitRequested += RequestedQuit;
            _input.OnRestartRequested += RequestRestart;
            _input.OnConfirmed += ConfirmRequest;
            _input.OnCanceled += CancelRequest;
        }

        private void Reset()
        {
            _field = new Field(Configuration.RowCount, Configuration.ColumnCount);
            _state = GameState.Normal;
            
            Score = 0;
        }

        private void Restart()
        {
            Reset();
            Start();
        }

        public async Task RunAsync()
        {
            _drawer.DrawLoader();
            
            await _saveProvider.InitializeAsync();
            
            BestScore = _saveProvider.BestScore;

            Start();
        }

        private void Start()
        {
            _field.PutNewValue();
            
            Update();
        }
        
        private void Update()
        {
            while (_state is not GameState.QuitAccepted)
            {
                if (_state is GameState.RestartAccepted)
                {
                    Restart();
                    return;
                }
                
                _field.Update();

                _drawer.Draw(this, _state, _field);
                _input.WaitForUserInput();
            }
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
                _saveProvider.BestScore = Score;
                _saveProvider.SaveAsync();
            }
        }
    }
}