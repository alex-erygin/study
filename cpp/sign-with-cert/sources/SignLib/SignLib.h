// SignLib.h

#pragma once



namespace SignLib {

	public ref class Signer
	{
		// TODO: здесь следует добавить свои методы для этого класса.
	public:
          void Sign(System::String^ signerName, System::String^ fileName, System::String^ signatureFileName);
		void Verify(System::String^ signerName, System::String^ signatureFileName, System::String^ dataFileName);

     private:
          void Sign();
	};
}