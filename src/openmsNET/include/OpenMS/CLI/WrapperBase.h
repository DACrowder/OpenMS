#pragma once

// standard includes for all wrapper classes
#include <msclr/lock.h>
#include <marshal.h>
#include <marshal_cppstd.h>

namespace OpenMSNET
{
  using OpenMSNET::ScopedPointer;

  /**
   * @brief Template for CLI wrapper classes
   * @tparam TUnmanaged the native unmanaged type being wrapped
   */

  template<class TUnmanaged>
  public ref class Managed
  {
    private:
      ScopedPointer<TUnmanaged>^ inst; // Managed handle to scoped pointer which points to the unmanaged instance
      bool isDisposed;

    protected:

      !Managed()
      {
        lock mtx(mIsDisposed);
        if (!mIsDisposed)
        {
          delete this->inst;
          this->isDisposed = true;
        }
      }

    public:
      Managed()
      {
        this->inst = ref new ScopedPointer<TUnmanaged*>(new TUnmanaged);
        this->isDisposed = false;
      }

      ~Managed()
      {
        !Managed();
      }
  };
}
