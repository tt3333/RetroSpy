using System;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    public sealed class DelayedControllerReader : IControllerReader, IDisposable
    {
        private readonly IControllerReader baseControllerReader;
        private readonly int delayInMilliseconds;

        public event EventHandler ControllerDisconnected;

        public event StateEventHandler ControllerStateChanged;

        public IControllerReader BaseControllerReader => baseControllerReader;
        public int DelayInMilliseconds => delayInMilliseconds;

        public DelayedControllerReader(IControllerReader baseControllerReader, int delayInMilliseconds)
        {
            this.baseControllerReader = baseControllerReader;
            this.delayInMilliseconds = delayInMilliseconds;

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
                await Task.Delay(delayInMilliseconds).ConfigureAwait(true);

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