/*
 * MinIO .NET Library for Amazon S3 Compatible Cloud Storage, (C) 2020 MinIO, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Minio.DataModel;

/// <summary>
///     Stores raw json events generated by ListenBucketNotifications
///     The Minio client doesn't depend on a JSON library so we can let
///     the caller use a library of their choice
/// </summary>
public class MinioNotificationRaw
{
    public string json;

    public MinioNotificationRaw(string json)
    {
        this.json = json;
    }
}

/// <summary>
///     Helper class to deserialize notifications generated
///     from MinioNotificaitonRaw by ListenBucketNotifications
/// </summary>
[Serializable]
public class MinioNotification
{
    public string Err { get; set; }
    public List<NotificationEvent> Records { get; set; }
}

public class NotificationEvent
{
    public string awsRegion { get; set; }
    public string eventName { get; set; }
    public string eventSource { get; set; }
    public string eventTime { get; set; }
    public string eventVersion { get; set; }
    public Dictionary<string, string> requestParameters { get; set; }
    public Dictionary<string, string> responseElements { get; set; }
    public EventMeta s3 { get; set; }
    public SourceInfo source { get; set; }
    public Identity userIdentity { get; set; }
}

[DataContract]
public class EventMeta
{
    [DataMember] public BucketMeta bucket { get; set; }

    [DataMember] public string configurationId { get; set; }

    [DataMember(Name = "object")]
    [JsonPropertyName("object")]
    public ObjectMeta objectMeta { get; set; } // C# won't allow the keyword 'object' as a name

    [DataMember] public string schemaVersion { get; set; }
}

public class ObjectMeta
{
    public string contentType { get; set; }
    public string etag { get; set; }
    public string key { get; set; }
    public string sequencer { get; set; }
    public int size { get; set; }
    public Dictionary<string, string> userMetadata { get; set; }
    public string versionId { get; set; }
}

public class BucketMeta
{
    public string arn { get; set; }
    public string name { get; set; }
    public Identity ownerIdentity { get; set; }
}

public class Identity
{
    public string principalId { get; set; }
}

public class SourceInfo
{
    public string host { get; set; }
    public string port { get; set; }
    public string userAgent { get; set; }
}