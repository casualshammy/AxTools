using SlimDX;

namespace AxTools.Classes.WoW.Overlay.Overlay
{
    public interface ICameraProvider
    {
        /// <summary> Gets the view.</summary>
        /// <value> The view.</value>
        Matrix View { get; }

        /// <summary> Gets the translation.</summary>
        /// <value> The translation.</value>
        Matrix Projection { get; }
    }
}
