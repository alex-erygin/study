namespace csharp hellothrift

struct SampleRequestDto {
	1: optional i32 Id,
	2: optional string Data,	
}

struct SampleResponseDto {
	1: optional string Data,
}

service MyService
{
	void Ping(),

	SampleResponseDto SendRequest(1: SampleRequestDto request),
}