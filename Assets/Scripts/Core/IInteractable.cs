namespace SLC.Bad4Business.Core
{
    public interface IInteractable
    {

        bool IsInteractable { get; }

        void OnInteract();
    }
}
