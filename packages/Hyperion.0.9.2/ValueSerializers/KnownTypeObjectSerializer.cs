﻿#region copyright
// -----------------------------------------------------------------------
//  <copyright file="KnownTypeObjectSerializer.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.IO;

namespace Hyperion.ValueSerializers
{
    public class KnownTypeObjectSerializer : ValueSerializer
    {
        private readonly ObjectSerializer _serializer;
        private readonly ushort _typeIdentifier;
        private readonly byte[] _typeIdentifierBytes;

        public KnownTypeObjectSerializer(ObjectSerializer serializer, ushort typeIdentifier)
        {
            _serializer = serializer;
            _typeIdentifier = typeIdentifier;
            _typeIdentifierBytes = BitConverter.GetBytes(typeIdentifier);
        }

        public override void WriteManifest(Stream stream, SerializerSession session)
        {
            stream.WriteByte(ObjectSerializer.ManifestIndex);
            stream.Write(_typeIdentifierBytes, 0, _typeIdentifierBytes.Length);
        }

        public override void WriteValue(Stream stream, object value, SerializerSession session)
        {
            _serializer.WriteValue(stream, value, session);
        }

        public override object ReadValue(Stream stream, DeserializerSession session)
        {
            return _serializer.ReadValue(stream, session);
        }

        public override Type GetElementType()
        {
            return _serializer.GetElementType();
        }
    }
}