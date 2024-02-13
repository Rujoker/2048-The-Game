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
        
        private Field Field;
        private GameState State = GameState.Normal;

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
            Field = new Field(Configuration.RowCount, Configuration.ColumnCount);
            State = GameState.Normal;
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
            Field.PutNewValue();
            
            Update();
        }
        
        private void Update()
        {
            while (State is not GameState.QuitAccepted)
            {
                if (State is GameState.RestartAccepted)
                {
                    Restart();
                    return;
                }
                
                Field.Update();

                _drawer.Draw(this, State, Field);
                _input.Wait();
            }
        }
        
        private void RequestedQuit()
        {
            State = GameState.QuitRequested;
        }

        private void RequestRestart()
        {
            State = GameState.RestartRequested;
        }

        private void ConfirmRequest()
        {
            State = State switch
            {
                GameState.RestartRequested => GameState.RestartAccepted,
                GameState.QuitRequested => GameState.QuitAccepted,
                _ => State
            };
        }

        private void CancelRequest()
        {
            State = GameState.Normal;
        }

        private void Move(Direction dir)
        {
            Field.Move(dir, out var score);
            
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