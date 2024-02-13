using _2048ConsoleEdition.Gameplay;

namespace _2048ConsoleEdition.Graphics;

public interface IDrawer
{
    void DrawLoader();
    void Draw(IScoreProvider scoreProvider, GameState gameState, Field field);
}