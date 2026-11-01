using MelonLoader;

namespace RealShop
{
    public partial class Config
    {
        private const bool ValueShowWelcomeOnNewGame = true;

        private readonly MelonPreferences_Entry<bool> _showWelcomeOnNewGame;

        public bool ShowWelcomeOnNewGame => _showWelcomeOnNewGame.Value;
    }
}
