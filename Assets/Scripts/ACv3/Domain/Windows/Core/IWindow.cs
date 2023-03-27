using UniRx;

namespace ACv3.Domain.Windows
{
    public interface IWindow
    {
        WindowId WindowId { get; }
    }
}