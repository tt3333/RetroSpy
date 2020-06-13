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

        public event StateEventHandler ControllerStateChanged;

        public event StateEventHandler ControllerStateChangedNoDelay;

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

        private async void BaseControllerReader_ControllerStateChanged(object sender, ControllerState e)
        {
            if (!disposedValue)
            {
                if (!legacyKeybindingBehavior)
                    ControllerStateChangedNoDelay?.Invoke(this, e);

                await Task.Delay(delayInMilliseconds);

                if(legacyKeybindingBehavior)
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
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

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