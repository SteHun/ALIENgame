using var game = new ALIENgame.Game1();
#if DEBUG
try
{
    game.Run();
}
catch
{
    game.CrashHandler();
    throw;
}
#else
game.Run();
#endif