using System;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    public sealed class DelayedControllerReader : IControllerReader, IDisposable
    {
        private readonly IControllerReader baseControllerReader;
        private readonly int delayInMilliseconds;
        private readonly bool legacyKeybindingBehavior;

        public event EventHandler ControllerDisconnected;

        public event EventHandler<ControllerStateEventArgs> ControllerStateChanged;

        public event EventHandler<ControllerStateEventArgs> ControllerStateChangedNoDelay;

        public IControllerReader BaseControllerReader => baseControllerReader;
        public int DelayInMilliseconds => delayInMilliseconds;
        public bool LegacyKeybindingBehavior => legacyKeybindingBehavior;
        public DelayedControllerReader(IControllerReader baseControllerReader, int delayInMilliseconds, bool legacyKeybindingBehavior)
        {
            this.baseControllerReader = baseControllerReader;
            this.delayInMilliseconds = delayInMilliseconds;
            this.legacyKeybindingBehavior = legacyKeybindingBehavior;

            BaseControllerReader.ControllerStateChanged += BaseControllerReader_ControllerStateChanged;
            BaseControllerReader.ControllerDisconnected += BaseControllerReader_ControllerDisconnected;
        }

        private void BaseControllerReader_ControllerDisconnected(object sender, EventArgs e)
        {
            ControllerDisconnected?.Invoke(this, e);
        }

        private async void BaseControllerReader_ControllerStateChanged(object sender, ControllerStateEventArgs e)
        {
            if (!disposedValue)
            {
                if (!legacyKeybindingBehavior)
                    ControllerStateChangedNoDelay?.Invoke(this, e);

                await Task.Delay(delayInMilliseconds).ConfigureAwait(true);

                if (legacyKeybindingBehavior)
                    ControllerStateChangedNoDelay?.Invoke(this, e);

                ControllerStateChanged?.Invoke(this, e);
            }
        }

        public void Finish()
        {
            if (!disposedValue)
            {
                BaseControllerReader.Finish();
            }
            Dispose();
        }

        #region IDisposable Support

        private bool disposedValue; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
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