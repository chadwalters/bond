// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Bond.Protocols
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using Newtonsoft.Json;

    [Reader(typeof(SimpleJsonReader))]
    public struct SimpleJsonWriter : IProtocolWriter
    {
        public const string NameAttribute = "JsonName";
        readonly JsonTextWriter writer;

        public bool OmitDefaultFields { get; set; }

        public SimpleJsonWriter(TextWriter writer)
        {
            this.writer = new JsonTextWriter(writer);
            OmitDefaultFields = true;
        }

        public SimpleJsonWriter(Stream stream)
        {
            writer = new JsonTextWriter(new StreamWriter(stream));
            OmitDefaultFields = true;
        }

        public void Flush()
        {
            writer.Flush();
        }

        #region IProtocolWriter

        public void WriteVersion()
        {
            throw new NotImplementedException();
        }

        #region Struct

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteStructBegin(Metadata metadata)
        {
            writer.WriteStartObject();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteStructEnd()
        {
            writer.WriteEndObject();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBaseBegin(Metadata metadata)
        {}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBaseEnd()
        {}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteFieldBegin(BondDataType type, ushort id, Metadata metadata)
        {
            string name;
            writer.WritePropertyName(metadata.attributes.TryGetValue(NameAttribute, out name) ? name : metadata.name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteFieldEnd()
        {}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteFieldOmitted(BondDataType type, ushort id, Metadata metadata)
        {
            if (!OmitDefaultFields)
            {
                Audit.ArgRule(!metadata.default_value.nothing, "Field set to nothing can't be serialized.");

                WriteFieldBegin(type, id, metadata);

                switch (type)
                {
                    case BondDataType.BT_BOOL:
                        WriteBool(0 != metadata.default_value.uint_value);
                        break;
                    case BondDataType.BT_UINT8:
                        WriteUInt8((byte)metadata.default_value.uint_value);
                        break;
                    case BondDataType.BT_UINT16:
                        WriteUInt16((UInt16)metadata.default_value.uint_value);
                        break;
                    case BondDataType.BT_UINT32:
                        WriteUInt32((UInt32)metadata.default_value.uint_value);
                        break;
                    case BondDataType.BT_UINT64:
                        WriteUInt64(metadata.default_value.uint_value);
                        break;
                    case BondDataType.BT_FLOAT:
                        WriteFloat((float)metadata.default_value.double_value);
                        break;
                    case BondDataType.BT_DOUBLE:
                        WriteDouble(metadata.default_value.double_value);
                        break;
                    case BondDataType.BT_STRING:
                        WriteString(metadata.default_value.string_value);
                        break;
                    case BondDataType.BT_LIST:
                    case BondDataType.BT_SET:
                    case BondDataType.BT_MAP:
                        WriteContainerBegin(0, type);
                        break;
                    case BondDataType.BT_INT8:
                        WriteInt8((sbyte)metadata.default_value.int_value);
                        break;
                    case BondDataType.BT_INT16:
                        WriteInt16((Int16)metadata.default_value.int_value);
                        break;
                    case BondDataType.BT_INT32:
                        WriteInt32((Int32)metadata.default_value.int_value);
                        break;
                    case BondDataType.BT_INT64:
                        WriteInt64(metadata.default_value.int_value);
                        break;
                    case BondDataType.BT_WSTRING:
                        WriteWString(metadata.default_value.wstring_value);
                        break;
                    default:
                        Throw.InvalidBondDataType(type);
                        break;
                }

                WriteFieldEnd();
            }
        }

        #endregion

        #region Containers

        public void WriteContainerBegin(int count, BondDataType keyType, BondDataType valueType)
        {
            writer.WriteStartArray();
        }

        public void WriteContainerBegin(int count, BondDataType elementType)
        {
            writer.WriteStartArray();
        }

        public void WriteContainerEnd()
        {
            writer.WriteEndArray();
        }

        #endregion

        #region Scalars

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBool(bool value)
        {
            writer.WriteValue(value);
        }

        public void WriteBytes(ArraySegment<byte> data)
        {
            var end = data.Offset + data.Count;
            for (var i = data.Offset; i != end; ++i)
            {
                writer.WriteValue((sbyte) data.Array[i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteDouble(double value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteFloat(float value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt16(short value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt32(int value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt64(long value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt8(sbyte value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt16(ushort value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt32(uint value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt64(ulong value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt8(byte value)
        {
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteString(string value)
        {
            // Other protocols depend on expressions such as value.Count to
            // throw an NRE if we've been asked to serialize a non-nullable
            // string field that is set to null. Newtonsoft.Json will
            // successfully serialize it as a JSON null (the unquoted text
            // null), so we need to check and throw explicitly before that.
            if (value == null)
            {
                throw new NullReferenceException(
                   "Attempted to serialize a null string. This may indicate a non-nullable string field that was set to null.");
            }
            writer.WriteValue(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteWString(string value)
        {
            // Other protocols depend on expressions such as value.Count to
            // throw an NRE if we've been asked to serialize a non-nullable
            // string field that is set to null. Newtonsoft.Json will
            // successfully serialize it as a JSON null (the unquoted text
            // null), so we need to check and throw explicitly before that.
            if (value == null)
            {
                throw new NullReferenceException(
                    "Attempted to serialize a null string. This may indicate a non-nullable string field that was set to null.");
            }
            writer.WriteValue(value);
        }

        #endregion

        #endregion
    }
}
