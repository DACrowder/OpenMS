# --------------------------------------------------------------------------
#                   OpenMS -- Open-Source Mass Spectrometry
# --------------------------------------------------------------------------
# Copyright The OpenMS Team -- Eberhard Karls University Tuebingen,
# ETH Zurich, and Freie Universitaet Berlin 2002-2018.
#
# This software is released under a three-clause BSD license:
#  * Redistributions of source code must retain the above copyright
#    notice, this list of conditions and the following disclaimer.
#  * Redistributions in binary form must reproduce the above copyright
#    notice, this list of conditions and the following disclaimer in the
#    documentation and/or other materials provided with the distribution.
#  * Neither the name of any author or any participating institution
#    may be used to endorse or promote products derived from this software
#    without specific prior written permission.
# For a full list of authors, refer to the file AUTHORS.
# --------------------------------------------------------------------------
# THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
# AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
# IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
# ARE DISCLAIMED. IN NO EVENT SHALL ANY OF THE AUTHORS OR THE CONTRIBUTING
# INSTITUTIONS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
# EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
# PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
# OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
# WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
# OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
# ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#
# --------------------------------------------------------------------------
# $Maintainer: Alex Crowder $
# $Authors: Alex Crowder $
# --------------------------------------------------------------------------

project("OpenMSNET")
cmake_minimum_required(VERSION 3.15.3 FATAL_ERROR)
if(NOT MSVC)
  message(FATAL_ERROR "MSVC required to build /clr C++/CLI")
elseif(NOT WIN32)
  message(FATAL_ERROR "Windows required for building /clr C++/CLI")
endif()
# --------------------------------------------------------------------------
#  set internal source and header cache vars
set(OpenMSNET_sources  CACHE INTERNAL "This variable should hold all OpenMS sources at the end of the config step" )
set(OpenMSNET_headers  CACHE INTERNAL "This variable should hold all OpenMS headers at the end of the config step" )

# --------------------------------------------------------------------------
# include header sources.cmake files
include(include/CLI/sources.cmake)
# --------------------------------------------------------------------------
# include implementation sources.cmake files
include(source/CLI/sources.cmake)

#------------------------------------------------------------------------------
# add the library
openms_add_library(TARGET_NAME OpenMSNET
                   SOURCE_FILES ${OpenMSNET_sources}
                   HEADER_FILES ${OpenMSNET_headers}
                   INTERNAL_INCLUDES ${PROJECT_SOURCE_DIR}/include ${PROJECT_BINARY_DIR}/include
                   EXTERNAL_INCLUDES ${OpenMS_INCLUDE_DIRECTORIES}
                   LINK_LIBRARIES OpenMS
                   DLL_EXPORT_PATH "OpenMS/OpenMSNET/"
                   CLR_TARGET true
                 )
# --------------------------------------------------------------------------
# link libraries for any external managed libraries/tools (CLR tools)
set(OpenMSNET_LIBRARIES OpenMSNET OpenMS CACHE INTERNAL "OpenMSNET managed CLI wrapper libraries" FORCE)

#------------------------------------------------------------------------------
## register paths for doxygen
openms_doc_path("${PROJECT_SOURCE_DIR}/include")