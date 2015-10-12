require 'bundler/setup'
require 'albacore'
require 'version_bumper'

version = bumper_version.to_s
buildNumber = "#{ENV['BUILD_NUMBER'] || '0'}"
buildVersion = "#{version}.#{buildNumber}"
basePath = File.expand_path(File.dirname(__FILE__))
buildPath = "#{basePath}/build"
assemblyInfoDir = basePath + "/src/MachineFormatGenerator/Properties"

desc "Default"
task :default => [:clean, :compile, :pack] do
  uploadNuget("#{ENV['USERPROFILE']}\\NuGet")
end

desc "Release"
task :release => [:clean, :compile, :pack] do
  uploadNuget("http://klondike.dev.eftdomain.net/api/")
end

desc "Build"
build :compile => [:solutionNugetPackages, :assemblyinfo] do |msb|
  msb.prop 'configuration', 'Release'
  msb.prop 'Platform', 'Any CPU'
  msb.prop 'outdir', buildPath
  msb.target = [ 'Clean', 'Build' ]
  msb.sln = "#{basePath}/src/MachineFormatGenerator.sln"
  msb.logging = 'normal'
end

desc "Restore solution level nuget packages."
nugets_restore :solutionNugetPackages do |restore|
  restore.out = 'src/packages'
  restore.exe = 'tools/NuGet.exe'
end

desc "AssemblyInfo"
asmver :assemblyinfo do |asm|
  FileUtils.mkdir(assemblyInfoDir) unless File.exists?(assemblyInfoDir)
  asm.file_path = "#{assemblyInfoDir}/AssemblyInfo.cs"
  asm.attributes assembly_version: version,
  assembly_file_version: buildVersion,
  assembly_company: "EFT Source",
  assembly_product: "EFT Machine Format Generation Library"
end

task :clean do
   FileUtils.rm_rf(Dir.glob(basePath + "/lastNuGet/*"))
   FileUtils.mkdir("#{basePath}/lastNuget") unless File.exists?("#{basePath}/lastNuget")
end

desc "create the nuget package"
nugets_pack :pack do |pack|
  pack.configuration = 'Release'
  pack.files = FileList['src/**/*.csproj'].
    exclude(/MachineFormatConsoleTester/)
  pack.out = 'lastNuget'
  pack.exe = 'tools/NuGet.exe'
  pack.with_metadata do |m|
    m.version = buildVersion
    m.authors = "Aaron Smith, Cam Wheeler"
    m.owners = "Smug Jenkins"
    m.description = "Library to generate specified machine files"
    m.title = "EFT Machine Format Generator"
  end
  pack.with_package do |p|
    p.add_file "#{buildPath}/MachineFormatGenerator.dll", "lib/net45"
  end
end

def uploadNuget (nugetRepo)
  puts "Uploading nuget package to #{nugetRepo}"
  `tools\\NuGet.exe push lastNuget\\*.nupkg -s "#{nugetRepo}"`
  raise unless $?.success?
end
