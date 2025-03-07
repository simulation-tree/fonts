using System;
using Unmanaged;

namespace Fonts.Components
{
    public struct IsFontRequest
    {
        public uint pixelSize;
        public ASCIIText256 address;
        public TimeSpan timeout;
        public TimeSpan duration;
        public Status status;

        public IsFontRequest(uint pixelSize, ASCIIText256 address, TimeSpan timeout)
        {
            this.pixelSize = pixelSize;
            this.address = address;
            this.timeout = timeout;
            this.duration = TimeSpan.Zero;
            this.status = Status.Submitted;
        }

        public readonly IsFontRequest BecomeLoaded()
        {
            IsFontRequest request = this;
            request.status = Status.Loaded;
            return request;
        }

        public enum Status : byte
        {
            Submitted,
            Loading,
            Loaded,
            NotFound
        }
    }
}