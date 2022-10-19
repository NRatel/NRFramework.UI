#include "il2cpp-config.h"
#include "vm/Assembly.h"
#include "vm/AssemblyName.h"
#include "vm/MetadataCache.h"
#include "vm/Runtime.h"
#include "vm-utils/VmStringUtils.h"
#include "il2cpp-tabledefs.h"
#include "il2cpp-class-internals.h"

// ==={{ hybridclr
#include "Baselib.h"
#include "Cpp/ReentrantLock.h"
#include "os/Atomic.h"
// ===}} hybridclr

#include <vector>
#include <string>

namespace il2cpp
{
namespace vm
{
    // ==={{ hybridclr
    static baselib::ReentrantLock s_assemblyLock;
    // copy on write
    static AssemblyVector s_emptyAssemblies;
    static AssemblyVector* s_Assemblies = &s_emptyAssemblies;
    static AssemblyVector* s_lastValidAssemblies = &s_emptyAssemblies;
    // ===}} hybridclr

    AssemblyVector* Assembly::GetAllAssemblies()
    {
        os::FastAutoLock lock(&s_assemblyLock);

        size_t validAssCount = 0;
        bool assemblyChange = false;
        for (AssemblyVector::const_iterator assIt = s_Assemblies->begin(); assIt != s_Assemblies->end(); ++assIt)
        {
            const Il2CppAssembly* ass = *assIt;
            if (ass->token == 0)
            {
                continue;
            }
            if (s_lastValidAssemblies->size() <= validAssCount || (*s_lastValidAssemblies)[validAssCount] != ass)
            {
                assemblyChange = true;
                break;
            }
            ++validAssCount;
        }
        if (assemblyChange)
        {
            s_lastValidAssemblies = new AssemblyVector();
            for (AssemblyVector::const_iterator assIt = s_Assemblies->begin(); assIt != s_Assemblies->end(); ++assIt)
            {
                const Il2CppAssembly* ass = *assIt;
                if (ass->token)
                {
                    s_lastValidAssemblies->push_back(ass);
                }
            }
        }
        return s_lastValidAssemblies;
    }

    const Il2CppAssembly* Assembly::GetLoadedAssembly(const char* name)
    {
        AssemblyVector& assemblies = *GetAllAssemblies();
        for (AssemblyVector::const_iterator assembly = assemblies.begin(); assembly != assemblies.end(); ++assembly)
        {
            if (strcmp((*assembly)->aname.name, name) == 0)
                return *assembly;
        }

        return NULL;
    }

    Il2CppImage* Assembly::GetImage(const Il2CppAssembly* assembly)
    {
        return assembly->image;
    }

    void Assembly::GetReferencedAssemblies(const Il2CppAssembly* assembly, AssemblyNameVector* target)
    {
        for (int32_t sourceIndex = 0; sourceIndex < assembly->referencedAssemblyCount; sourceIndex++)
        {
            const Il2CppAssembly* refAssembly = MetadataCache::GetReferencedAssembly(assembly, sourceIndex);

            target->push_back(&refAssembly->aname);
        }
    }

    static bool ends_with(const char *str, const char *suffix)
    {
        if (!str || !suffix)
            return false;

        const size_t lenstr = strlen(str);
        const size_t lensuffix = strlen(suffix);
        if (lensuffix >  lenstr)
            return false;

        return strncmp(str + lenstr - lensuffix, suffix, lensuffix) == 0;
    }

    const Il2CppAssembly* Assembly::Load(const char* name)
    {
// ==={{ hybridclr
        const Il2CppAssembly* loadedAssembly = MetadataCache::GetAssemblyByName(name);
        if (loadedAssembly)
        {
            return loadedAssembly;
        }

        if (!ends_with(name, ".dll") && !ends_with(name, ".exe"))
        {
            const size_t len = strlen(name);
            char *tmp = new char[len + 5];

            memset(tmp, 0, len + 5);

            memcpy(tmp, name, len);
            memcpy(tmp + len, ".dll", 4);

            loadedAssembly = MetadataCache::GetAssemblyByName(tmp);

            if (!loadedAssembly)
            {
                memcpy(tmp + len, ".exe", 4);
                loadedAssembly = MetadataCache::GetAssemblyByName(tmp);
            }

            memcpy(tmp + len, ".dll", 4);
            loadedAssembly = MetadataCache::LoadAssemblyByName(tmp);

            delete[] tmp;

            return loadedAssembly;
        }
        else
        {
            return MetadataCache::LoadAssemblyByName(name);
        }
// ===}} hybridclr
    }

    void Assembly::Register(const Il2CppAssembly* assembly)
    {
// ==={{ hybridclr
        os::FastAutoLock lock(&s_assemblyLock);

        AssemblyVector* oldAssemblies = s_Assemblies;

        // TODO IL2CPP_MALLOC ???
        AssemblyVector* newAssemblies = oldAssemblies ? new AssemblyVector(*oldAssemblies) : new AssemblyVector();
        newAssemblies->push_back(assembly);
        s_Assemblies = newAssemblies;
        // can't delete oldAssemblies because may be using by other thread
        if (oldAssemblies)
        {
            // can't delete
            // delete oldAssemblies;
        }
// ===}} hybridclr
    }

    void Assembly::ClearAllAssemblies()
    {
// ==={{ hybridclr
        os::FastAutoLock lock(&s_assemblyLock);
        AssemblyVector* oldAssemblies = s_Assemblies;
        s_Assemblies = nullptr;
        if (oldAssemblies)
        {
            // TODO ???
        }
// ===}} hybridclr
    }

    void Assembly::Initialize()
    {
    }
} /* namespace vm */
} /* namespace il2cpp */
