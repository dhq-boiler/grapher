﻿using boilersGraphics.Helpers;
using boilersGraphics.Models;
using boilersGraphics.Views;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace boilersGraphics.ViewModels
{
    class SettingViewModel : BindableBase, IDialogAware, IDisposable
    {
        private IDialogService dlgService;
        private bool disposedValue;
        private CompositeDisposable _disposables = new CompositeDisposable();

        public ReactiveProperty<ReactiveCommand> OkCommand { get; set; } = new ReactiveProperty<ReactiveCommand>();
        public ReactiveCommand CancelCommand { get; set; }
        public ReactiveCommand ChangeCanvasBackgroundCommand { get; set; } = new ReactiveCommand();

        public ReactiveProperty<Models.Setting> EditTarget { get; set; } = new ReactiveProperty<Models.Setting>();

        public SettingViewModel(IDialogService dialogService)
        {
            this.dlgService = dialogService;
            EditTarget.Value = new Models.Setting();
            CancelCommand = new ReactiveCommand();
            CancelCommand.Subscribe(_ =>
            {
                var ret = new DialogResult(ButtonResult.Cancel, null);
                RequestClose.Invoke(ret);
            })
            .AddTo(_disposables);
            ChangeCanvasBackgroundCommand.Subscribe(_ =>
            {
                IDialogResult result = null;
                this.dlgService.ShowDialog(nameof(ColorPicker),
                                           new DialogParameters()
                                           {
                                               {
                                                   "ColorExchange",
                                                   new ColorExchange()
                                                   {
                                                       Old = EditTarget.Value.CanvasBackground.Value
                                                   }
                                               }
                                           },
                                           ret => result = ret);
                if (result != null)
                {
                    var exchange = result.Parameters.GetValue<ColorExchange>("ColorExchange");
                    if (exchange != null)
                    {
                        EditTarget.Value.CanvasBackground.Value = exchange.New.Value;
                    }
                }
            })
            .AddTo(_disposables);
            EditTarget.Subscribe(_ =>
            {
                OkCommand.Value = EditTarget.Value
                             .Width
                             .CombineLatest(EditTarget.Value.Height, (x, y) => x * y)
                             .Select(x => x > 0)
                             .ToReactiveCommand();
                OkCommand.Value.Subscribe(__ =>
                {
                    var parameters = new DialogParameters() { { "Setting", EditTarget.Value } };
                    var ret = new DialogResult(ButtonResult.OK, parameters);
                    RequestClose.Invoke(ret);
                })
                .AddTo(_disposables);
            })
            .AddTo(_disposables);
        }

        public string Title => "設定";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            EditTarget.Value = parameters.GetValue<Models.Setting>("Setting");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _disposables.Dispose();
                }

                _disposables = null;
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
