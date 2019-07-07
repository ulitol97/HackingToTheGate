namespace Game.UI.Menus
{
    /// <summary>
    /// The interface IMenu defines the functionality any game menu must have, including quiting the menu and showing
    /// the user manual.
    /// </summary>
    public interface IMenu
    {
        /// <summary>
        /// Close the menu and return to a previous screen.
        /// </summary>
        void QuitMenu();

        /// <summary>
        /// Show the player manual.
        /// </summary>
        void ToggleHelp();
    }
}
