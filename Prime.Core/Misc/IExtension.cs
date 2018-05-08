﻿using System;
using LiteDB;
using Prime.Base;

namespace Prime.Core
{
    public interface IExtension : IUniqueIdentifier<ObjectId>
    {
        string Title { get; }

        Version Version { get; }
    }
}