### the directory name
set(directory "include/OpenMSNET/CLI")

### list all header files of the directory here
set(sources_list_h
	"ScopedPointer.h"
	"DPosition.h"
	"Peak1D.h"
)

### add path to the filenames
set(sources_h)
foreach(i ${sources_list_h})
	list(APPEND sources_h ${directory}/${i})
endforeach(i)
set(OpenMSNET_headers ${OpenMSNET_headers} ${sources_h})
source_group("Header Files\\OpenMSNET\\CLI" FILES ${sources_h})


