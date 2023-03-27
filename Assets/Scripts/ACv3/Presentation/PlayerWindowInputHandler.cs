using System;
using ACv3.Domain.Windows;
using ACv3.Extensions;
using ACv3.Presentation.Inputs;
using ACv3.UseCase.Window;
using UniRx;
using Zenject;

namespace ACv3.Presentation
{
    public class PlayerWindowInputHandler : IInitializable, IDisposable
    {
        readonly IInputController inputController;
        readonly WindowService windowService;
        readonly CompositeDisposable disposable = new();

        [Inject]
        PlayerWindowInputHandler(IInputController inputController, WindowService windowService)
        {
            this.inputController = inputController;
            this.windowService = windowService;
        }

        void IInitializable.Initialize()
        {
            ObservableExt.SmartAny(
                    inputController.OnOpenPlayerWindowRequested(),
                    inputController.OnCloseWindowRequested())
                .Subscribe(winType =>
                {
                    if (winType == ObservableExt.WinType.Left)
                    {
                        windowService.Open(WindowId.PlayerWindow);
                        return;
                    }

                    if (winType == ObservableExt.WinType.Right)
                    {
                        windowService.CloseAll();
                        return;
                    }

                    if (windowService.IsOpened(WindowId.PlayerWindow))
                    {
                        windowService.CloseAll();
                    }
                    else
                    {
                        windowService.Open(WindowId.PlayerWindow);
                    }
                })
                .AddTo(disposable);
        }

        void IDisposable.Dispose()
        {
            disposable.Dispose();
        }
    }
}