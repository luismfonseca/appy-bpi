// TODO: add by discover 20130111

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing.Common;

namespace ZXing.qrcode.Internal
{
    /// <summary>
    /// The class contains all information about the QRCode which was found
    /// </summary>
    public class QRCodeDetectorResult : DetectorResult
    {
        /// <summary>
        /// ModuleSize
        /// </summary>
        public float ModuleSize { get; private set; }

        /// <summary>
        /// Dimension
        /// </summary>
        public int Dimension { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="QRCodeDetectorResult"/> class.
        /// </summary>
        /// <param name="bits"></param>
        /// <param name="points"></param>
        /// <param name="moduleSize"></param>
        /// <param name="dimension"></param>
        public QRCodeDetectorResult(BitMatrix bits, ResultPoint[] points, float moduleSize, int dimension)
            : base(bits, points)
        {
            ModuleSize = moduleSize;
            Dimension = dimension;
        }
    }
}
