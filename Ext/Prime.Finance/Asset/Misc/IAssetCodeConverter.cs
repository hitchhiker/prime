﻿namespace Prime.Finance
{
    public interface IAssetCodeConverter
    {
        string ToLocalCode(string remoteCode);

        string ToRemoteCode(Asset localAsset);
    }
}