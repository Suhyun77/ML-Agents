namespace Anipen
{
    public interface ISelectable
    {
        void OnSelect();
        void OnDeselect();

        bool IsSelectable();
    }

}
