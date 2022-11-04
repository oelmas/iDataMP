using System;
using System.Drawing;
using System.IO;

namespace MissionPlanner.Utilities
{
    public class OpticalFlow : IDisposable
    {
        private readonly MAVLinkInterface _mav;

        private MemoryStream imageBuffer = new MemoryStream();
        // since its not listed in the docs
        // https://github.com/PX4/Flow/blob/4a314cfdb099aed9795b825e7518203771207fbb/src/modules/flow/dcmi.c#L292

        // size of incomming data
        private MAVLink.mavlink_data_transmission_handshake_t msgDataTransmissionHandshake;

        // data from handshake
        private MAVLink.mavlink_encapsulated_data_t msgEncapsulatedData;

        private readonly int subDataTrans;
        private readonly int subEncapData;

        public OpticalFlow(MAVLinkInterface mav, byte sysid, byte compid)
        {
            _mav = mav;

            subDataTrans = mav.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.DATA_TRANSMISSION_HANDSHAKE, ReceviedPacket,
                sysid, compid);
            subEncapData =
                mav.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.ENCAPSULATED_DATA, ReceviedPacket, sysid, compid);
        }

        public void Dispose()
        {
            Close();
        }

        public event EventHandler<ImageEventHandle> newImage;

        public void CalibrationMode(bool on_off = false)
        {
            if (on_off)
                _mav.setParam("VIDEO_ONLY", 1);
            else
                _mav.setParam("VIDEO_ONLY", 0);
        }

        private bool ReceviedPacket(MAVLink.MAVLinkMessage arg)
        {
            if (arg.msgid == (byte)MAVLink.MAVLINK_MSG_ID.DATA_TRANSMISSION_HANDSHAKE)
            {
                var packet = arg.ToStructure<MAVLink.mavlink_data_transmission_handshake_t>();
                msgDataTransmissionHandshake = packet;
                imageBuffer.Close();
                imageBuffer = new MemoryStream((int)packet.size);
            }
            else if (arg.msgid == (byte)MAVLink.MAVLINK_MSG_ID.ENCAPSULATED_DATA)
            {
                var packet = arg.ToStructure<MAVLink.mavlink_encapsulated_data_t>();
                msgEncapsulatedData = packet;
                var start = msgDataTransmissionHandshake.payload * msgEncapsulatedData.seqnr;
                var left = (int)msgDataTransmissionHandshake.size - start;
                var writeamount = Math.Min(msgEncapsulatedData.data.Length, left);

                imageBuffer.Seek(start, SeekOrigin.Begin);
                imageBuffer.Write(msgEncapsulatedData.data, 0, writeamount);

                // we have a complete image
                if (msgEncapsulatedData.seqnr + 1 == msgDataTransmissionHandshake.packets)
                    using (
                        var bmp = new Bitmap(msgDataTransmissionHandshake.width, msgDataTransmissionHandshake.height))
                    {
                        SetGrayscalePalette(bmp);

                        if (imageBuffer.Length > msgDataTransmissionHandshake.size)
                            return true;

                        var buffer = imageBuffer.ToArray();

                        var a = 0;
                        foreach (var b in buffer)
                        {
                            bmp.SetPixel(a % bmp.Width, a / bmp.Width, bmp.Palette.Entries[b]);
                            a++;
                        }

                        if (newImage != null)
                            newImage(this, new ImageEventHandle(bmp));
                        //bmp.Save("test.bmp", ImageFormat.Bmp);
                    }
            }

            return true;
        }

        public void SetGrayscalePalette(Bitmap bmp)
        {
            var _palette = bmp.Palette;
            for (var i = 0; i < 256; i++)
                _palette.Entries[i] = Color.FromArgb(255, i, i, i);
            bmp.Palette = _palette;
        }

        public void Close()
        {
            _mav.UnSubscribeToPacketType(subDataTrans);
            _mav.UnSubscribeToPacketType(subEncapData);

            imageBuffer.Close();
        }

        public class ImageEventHandle : EventArgs
        {
            public ImageEventHandle(Image bmp)
            {
                Image = bmp;
            }

            public Image Image { get; set; }
        }
    }
}