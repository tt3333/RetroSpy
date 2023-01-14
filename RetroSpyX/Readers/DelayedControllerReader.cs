using System;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    public sealed class DelayedControllerReader : IControllerReader, IDisposable
    {
        public event EventHandler? ControllerDisconnected;

        public event EventHandler<ControllerStateEventArgs>? ControllerStateChanged;

        public event EventHandler<ControllerStateEventArgs>? ControllerStateChangedNoDelay;

        public IControllerReader? BaseControllerReader { get; }
        public int DelayInMilliseconds { get; }
        public bool LegacyKeybindingBehavior { get; }
        public DelayedControllerReader(IControllerReader? baseControllerReader, int delayInMilliseconds, bool legacyKeybindingBehavior)
        {
            BaseControllerReader = baseControllerReader;
            DelayInMilliseconds = delayInMilliseconds;
            LegacyKeybindingBehavior = legacyKeybindingBehavior;

            if (BaseControllerReader != null)
            {
                BaseControllerReader.ControllerStateChanged += BaseControllerReader_ControllerStateChanged;
                BaseControllerReader.ControllerDisconnected += BaseControllerReader_ControllerDisconnected;
            }
        }

        private void BaseControllerReader_ControllerDisconnected(object? sender, EventArgs e)
        {
            ControllerDisconnected?.Invoke(this, e);
        }

        private async void BaseControllerReader_ControllerStateChanged(object? sender, ControllerStateEventArgs e)
        {
            if (!disposedValue)
            {
                if (!LegacyKeybindingBehavior)
                {
                    ControllerStateChangedNoDelay?.Invoke(this, e);
                }

                await Task.Delay(DelayInMilliseconds).ConfigureAwait(true);

                if (LegacyKeybindingBehavior)
                {
                    ControllerStateChangedNoDelay?.Invoke(this, e);
                }

                ControllerStateChanged?.Invoke(this, e);
            }
        }

        public void Finish()
        {
            if (!disposedValue)
            {
                BaseControllerReader?.Finish();
            }
            Dispose();
        }

        #region IDisposable Support

        private bool disposedValue; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (BaseControllerReader != null && disposing)
                {
                    BaseControllerReader.ControllerStateChanged -= BaseControllerReader_ControllerStateChanged;
                    BaseControllerReader.ControllerDisconnected -= BaseControllerReader_ControllerDisconnected;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable Support
    }
}