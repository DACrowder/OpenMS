### the directory name
set(directory include/OpenMS/CLI)

### list all header files of the directory here
set(sources_list_h
	ScopedPointer.h
	DPosition.h
)

### add path to the filenames
set(sources_h)
foreach(i ${sources_list_h})
	list(APPEND sources_h ${directory}/${i})
endforeach(i)

source_group("Header Files\\OpenMS\\CLI" FILES ${sources_h})
set_source_files_properties(${directory}/sources.cmake PROPERTIES HEADER_FILE_ONLY TRUE)
set(OpenMSNET_sources_h ${OpenMSNET_sources_h} ${sources_h})

