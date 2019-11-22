#pragma once

// standard includes for all wrapper classes
#include <msclr/lock.h>
#include <marshal.h>
#include <marshal_cppstd.h>

namespace OpenMSNET
{
  template<typename TUnmanaged>
  public ref class WrapperBase
  {
    private:
      ScopedPointer<TUnmanaged>^ inst; // Managed handle to scoped pointer which points to the unmanaged instance


    protected:
      !WrapperBase()

    public:
      ~WrapperBase() { !WrapperBase(); }
      WrapperBase() { this->inst = gcnew ScopedPointer<TUnmanaged*>; }
  };
}
