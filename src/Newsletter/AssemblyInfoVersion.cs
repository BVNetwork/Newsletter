using System.Reflection;

#if CMS9
    [assembly: AssemblyVersion("9.0.0")]
#elif CMS10
    [assembly: AssemblyVersion("10.0.0")]
#else 
    [assembly: AssemblyVersion("11.4.2")]
#endif