#pragma once
#include <CLI/DPosition2.h>
namespace OpenMSNET
{
	DPosition2::DPosition2()
	{
		this->mIsDisposed = false;
		this->inst = new OpenMS::DPosition<2>;
	}

	DPosition2::DPosition2(OpenMS::DPosition<2>& dp)
	{
		this->mIsDisposed = false;
		this->inst = new OpenMS::DPosition<2>(dp);
	}

	DPosition2::DPosition2(OpenMS::DPosition<2>&& dp)
	{
		this->mIsDisposed = false;
		this->inst = new OpenMS::DPosition<2>(dp);
	}

	DPosition2::DPosition2(double x, double y)
	{
		this->mIsDisposed = false;
		this->inst = new OpenMS::DPosition<2>(x, y);
	}

	DPosition2::~DPosition2()
	{
		this->!DPosition2();
	}

	DPosition2::!DPosition2()
	{
		if (!this->mIsDisposed)
		{
			delete this->inst;
			this->mIsDisposed = true;
		}
	}

	bool DPosition2::DPosition2::SpatiallyLessEqual(DPosition2^ point)
	{
		bool yes = this->inst->spatiallyLessEqual(*(point->inst));
		return yes;
	}

	bool DPosition2::DPosition2::SpatiallyGreaterEqual(DPosition2^ point)
	{
		bool yes = this->inst->spatiallyGreaterEqual(*(point->inst));
		return yes;
	}

	void DPosition2::X::set(double x)
	{
		this->inst->setX(x);
	}

	double DPosition2::X::get()
	{
		double x = this->inst->getX();
		System::Console::WriteLine(x);
		return x;
	}

	void DPosition2::Y::set(double y)
	{
		this->inst->setY(y);
	}

	double DPosition2::Y::get()
	{
		return this->inst->getY();
	}
}