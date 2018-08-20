using NerdyMishka.KeePass.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.KeePass
{
    public class KeePassFileHeaderInformation
    {
        private byte randomByteGeneratorCryptoType;
        private IRandomByteGeneratorEngine randomByteGenerator;
        private byte[] randomByteGeneratorCryptoKey;

        internal const uint Signature1 = 0x9AA2D903;
        internal const uint Signature2 = 0xB54BFB67;
        internal const uint Version = 0x00030001;
        internal const uint Mask = 0xFFFF0000;

        public byte[] DatabaseCipherId { get; set; }

        public byte DatabaseCompression { get; set; }

        public byte[] DatabaseCipherKeySeed { get; set; }

        public byte[] MasterKeyHashKey { get; set; }

        public long MasterKeyHashRounds { get; set; }
        
        public byte[] DatabaseCipherIV { get; set; }

        public byte[] RandomByteGeneratorCryptoKey
        {
            get { return this.randomByteGeneratorCryptoKey; }
            set
            {
                this.randomByteGeneratorCryptoKey = value;
                this.randomByteGenerator = null;
            }
        }

        public byte[] HeaderByteMarks { get; set; }
      
        public byte RandomByteGeneratorCryptoType

        {
            get { return this.randomByteGeneratorCryptoType; }
            set
            {
                this.randomByteGeneratorCryptoType = value;
                this.randomByteGenerator = null;
            }
        }

        public byte[] Hash { get; set; }

        public IRandomByteGeneratorEngine RandomByteGenerator
        {
            get
            {
                if(this.randomByteGenerator == null)
                {
                    this.randomByteGenerator = RandomByteGeneratorFactory.GetGenerator(this.RandomByteGeneratorCryptoType);
                    this.randomByteGenerator.Initialize(this.RandomByteGeneratorCryptoKey);
                }

                return this.randomByteGenerator;
            }
        }
    
        public void GenerateValues()
        {
            if (this.MasterKeyHashRounds == 0)
                this.MasterKeyHashRounds = 6000;

            this.DatabaseCipherId = new KeePassCompliantAesEngine().Id;
            this.DatabaseCompression = (byte)1;
            this.RandomByteGeneratorCryptoType = (byte)2;
            var rng = System.Security.Cryptography.RandomNumberGenerator.Create();

            var bytes = new byte[32];

            rng.GetBytes(bytes);
            this.DatabaseCipherKeySeed = bytes;

            rng.GetBytes(bytes);
            this.MasterKeyHashKey = bytes;
            

            rng.GetBytes(bytes);
            this.RandomByteGeneratorCryptoKey = bytes;
           

            rng.GetBytes(bytes);
            this.HeaderByteMarks = bytes;
           

            bytes = new byte[16];
            rng.GetBytes(bytes);
            this.DatabaseCipherIV = bytes;
        }



        public void Write(Stream stream)
        {
            stream.Write(Signature1);
            stream.Write(Signature2);
            stream.Write(Version);

            var r = (byte)'\r';
            var n = (byte)'\n';

            stream.WriteHeader(KeePassFileHeaderFields.DatabaseCipherId, this.DatabaseCipherId);
            stream.WriteHeader(KeePassFileHeaderFields.DatabaseCipherKeySeed, this.DatabaseCipherKeySeed);
            stream.WriteHeader(KeePassFileHeaderFields.MasterKeyHashSeed, this.MasterKeyHashKey);
            stream.WriteHeader(KeePassFileHeaderFields.MasterKeyHashRounds, BitConverter.GetBytes((ulong)this.MasterKeyHashRounds));
            stream.WriteHeader(KeePassFileHeaderFields.DatabaseCipherIV, this.DatabaseCipherIV);
            stream.WriteHeader(KeePassFileHeaderFields.RandomBytesCryptoKey, this.RandomByteGeneratorCryptoKey);
            stream.WriteHeader(KeePassFileHeaderFields.HeaderByteMark, this.HeaderByteMarks);
            stream.WriteHeader(KeePassFileHeaderFields.RandomBytesCryptoType, BitConverter.GetBytes((uint)2));
            stream.WriteHeader(KeePassFileHeaderFields.DatabaseCompression, BitConverter.GetBytes((uint)1));
            stream.WriteHeader(KeePassFileHeaderFields.EndOfHeader,  new byte[] {r,n,r,n});
        }

        public void Read(Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                byte[] signature1Bytes = stream.ReadBytes(4);
                byte[] signature2Bytes = stream.ReadBytes(4);

                uint signature1 = signature1Bytes.ToUInt();
                uint signature2 = signature2Bytes.ToUInt();

                bool pass = signature1 == Signature1 && signature2 == Signature2;
                if (!pass)
                    throw new FormatException("Database has incorrect file signature");

                byte[] versionBytes = stream.ReadBytes(4);
                uint version = versionBytes.ToUInt();

                if ((version & Mask) > (Version & Mask))
                    throw new FormatException("The file version is unsupported");

                ms.Write(signature1Bytes);
                ms.Write(signature2Bytes);
                ms.Write(versionBytes);

                while(this.ReadNext(stream, ms))
                {
                    // Add Logging Statement
                }

                this.Hash = ms.ToSHA256Hash();
            }
        }

        protected bool ReadNext(Stream input, Stream output)
        {
            int nextByte = input.ReadByte();
            if (nextByte == -1)
                throw new EndOfStreamException();

            byte fieldIdByte = (byte)nextByte;
            output.WriteByte(fieldIdByte);

            KeePassFileHeaderFields field = (KeePassFileHeaderFields)fieldIdByte;

            byte[] sizeBytes = input.ReadBytes(2);
            ushort size = sizeBytes.ToUShort();
           

            byte[] data = null;
            if (size > 0)
                data = input.ReadBytes(size);

            output.Write(sizeBytes);
            output.Write(data);

            switch (field)
            {
                case KeePassFileHeaderFields.EndOfHeader:
                    return false;

                case KeePassFileHeaderFields.DatabaseCipherId:
                    this.DatabaseCipherId = data;
                    break;
                case KeePassFileHeaderFields.DatabaseCompression:
                    if (data.Length < 1)
                        throw new Exception("Invalid Header");
                    byte flag = data[0];
                    this.DatabaseCompression = data[0];
                    break;
                case KeePassFileHeaderFields.DatabaseCipherKeySeed:
                    this.DatabaseCipherKeySeed = data;
                    break;
                case KeePassFileHeaderFields.MasterKeyHashSeed:
                    this.MasterKeyHashKey = data;
                    break;
                case KeePassFileHeaderFields.MasterKeyHashRounds:
                    this.MasterKeyHashRounds = data.ToInt32();
                    break;
                case KeePassFileHeaderFields.DatabaseCipherIV:
                    this.DatabaseCipherIV = data;
                    break;
                case KeePassFileHeaderFields.RandomBytesCryptoKey:
                    this.RandomByteGeneratorCryptoKey = data;
                   
                    break;
                case KeePassFileHeaderFields.HeaderByteMark:
                    this.HeaderByteMarks = data;
                    break;

                case KeePassFileHeaderFields.RandomBytesCryptoType:
                    if (data.Length < 1)
                        throw new Exception("Invalid Header");
                    this.RandomByteGeneratorCryptoType = data[0];
                    break;
                default:
                    return false;
          
            }

            return true;
        }
    }
}
