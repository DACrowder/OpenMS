#pragma once
#include <msclr/marshal.h>
#include <msclr/marshal_cppstd.h>
#include <OpenMS/DATASTRUCTURES/DPosition.h>
#include <OpenMSNET/CLI/ScopedPointer.h>

namespace OpenMSNET
{
  template<System::UInt64 D, typename TCoordinate = double>
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
        this->mIsDisposed = true;
      }
    }

    DPosition()
    {
       this->mIsDisposed = false;
       this->inst = ref new OpenMSNET::ScopedPointer(
          new OpenMS::DPosition<D, TCoordinate>()
       );
    }

    DPosition(OpenMS::DPosition<D, TCoordinate>& dp)
    {
      this->mIsDisposed = false;
      this->inst = ref new OpenMSNET::ScopedPointer<OpenMS::DPosition<D, TCoordinate>>(
          new OpenMS::DPosition<D, TCoordinate>(dp)
      );
    }

    DPosition(OpenMS::DPosition<D, TCoordinate>&& dp)
    {
      this->mIsDisposed = false;
      this->inst = ref new OpenMSNET::ScopedPointer<OpenMS::DPosition<D, TCoordinate>>(
          new OpenMS::DPosition<D, TCoordinate>(dp)
      );
    }

    DPosition(TCoordinate x)
    {
      mIsDisposed = false;
      this->inst = ref new OpenMSNET::ScopedPointer<OpenMS::DPosition<D, TCoordinate>>(
          new OpenMS::DPosition<D, TCoordinate>(x)
        );
    }

    property TCoordinate X;
    property TCoordinate Y;

    CoordinateType operator[](int i) const
    {
      return msclr::interop::marshal_as<TCoordinate>(this->inst->get()[i]);
    }

    CoordinateType% operator[](int i)
    {
      return msclr::interop::marshal_as<TCoordinate%>(this->inst->get()[i]);
    }

    bool operator==(DPosition% point) { return this->inst->get() == point.inst->get(); }
    bool operator!=(DPosition% point) { return this->inst->get() != point.inst->get(); }
    bool operator<(DPosition% point)  { return this->inst->get() < point.inst->get();  }
    bool operator<=(DPosition% point) { return this->inst->get() <= point.inst->get(); }
    bool operator>(DPosition% point)  { return this->inst->get() <= point.inst->get(); }
    bool operator>=(DPosition% point) { return this->inst->get() <= point.inst->get(); }

    bool SpatiallyLessEqual(DPosition% point) { return this->inst->get()->spatiallyLessEqual(point); }
    bool SpatiallyGreaterEqual(DPosition% point) { return this->inst->get()->spatiallyGreaterEqual(point); }

    DPosition^ operator+(DPosition% point)
    {
      OpenMS::DPosition dp = this->inst->get() + point.inst()->get();
      return ref new DPosition(&this->inst->get() + point.inst()->get());
    }

    DPosition%^ operator+=(DPosition% point)
    {
      this->inst->get() += point.inst->get();
      return %this;
    }

    DPosition^ operator-(DPosition% point)
    {
      OpenMS::DPosition dp = this->inst->get() - point.inst()->get();
      return ref new DPosition(&this->inst->get() - point->inst()->get());
    }

    DPosition%^ operator-=(DPosition% point)
    {
      this->inst->get() -= point.inst->get();
      return %this;
    }

    DPosition^ operator-()
    {
      return ref new DPosition(-this->inst->get());
    }

    TCoordinate operator*(DPosition% point)
    {
      return msclr::interop::marshal_as<TCoordinate>(this->inst->get() * point.inst->get());
    }

    DPosition%^ operator*=(TCoordinate scalar)
    {
      this->inst->get() *= point.inst->get();
      return %this;
    }

    DPosition%^ operator/=(TCoordinate scalar)
    {
      this->inst->get() /= point.inst->get();
      return %this;
    }

    static property DPosition^ Zero
    {
      DPosition^ get()
      {
        DPosition^ dp = new DPosition(*OpenMS::DPosition::zero());
        return dp;
      }
    }

    static property DPosition^ MinPositive
    {
      DPosition^ get()
      {
        DPosition^ dp = new DPosition(*OpenMS::DPosition::minPositive());
        return dp;
      }
    }

    static property DPosition^ MinNegative
    {
      DPosition^ get()
      {
        DPosition^ dp = new DPosition(*OpenMS::DPosition::minNegative());
        return dp;
      }
    }

    static property DPosition^ MaxPositive
    {
      DPosition^ get()
      {
        DPosition^ dp = new DPosition(*OpenMS::DPosition::maxPositive());
        return dp;
      }
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
};

