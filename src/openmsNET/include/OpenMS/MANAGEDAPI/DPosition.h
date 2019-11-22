#pragma once
#include <msclr>
#include <marshal.h>
#include <marshal_cppstd.h>
#include <OpenMS/DATASTRUCTURES/DPosition.h>
#include <ScopedPointer.h>

namespace OpenMSNET
{
  template<UInt D, typename TCoordinate = double>
  public ref class DPosition
  {
  private:

    ScopedPointer<OpenMS::DPosition<D, TCoordinateType>>^ inst; // Managed handle to scoped pointer which points to the unmanaged instance
    bool mIsDisposed;

  public:

    ~DPosition()
    {
      lock mtx(mIsDisposed);
      if (!mIsDisposed)
      {
        delete this->inst;
        mIsDisposed = true;
      }
    }

    DPosition()
    {
       mIsDisposed = false;
       this->inst = ref new OpenMSNET::ScopedPointer<OpenMS::DPosition<D, TCoordinateType>>(
          new OpenMS::DPosition<D, TCoordinate>()
       );
    }

    DPosition(TCoordinateType x)
    {
      mIsDisposed = false;
      this->inst = ref new OpenMSNET::ScopedPointer<OpenMS::DPosition<D, TCoordinateType>>(
          new OpenMS::DPosition<D, TCoordinate>(x)
        );
    }

    CoordinateType operator[]( index)
    {

    }



  protected:

    !DPosition()
    {
      msclr::lock::lock mtx(mIsDisposed);
      if (!mIsDisposed)
      {
        delete this->inst->get();
        mIsDisposed = true;
      }
    }

  };
}

