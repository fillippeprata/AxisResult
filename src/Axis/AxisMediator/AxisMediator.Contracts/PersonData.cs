using Axis;

namespace AxisMediator.Contracts;

public readonly record struct PersonData(AxisIdentity AxisIdentity, string DisplayName, string PictureProxyUrl, string LanguageId);
