import { SidebarTrigger } from "@/components/ui/sidebar";
import { Separator } from "@/components/ui/separator";

function Header() {
  return (
    <header className="flex h-14 shrink-0 items-center gap-2 border-b px-4 md:hidden">
      <SidebarTrigger />
      <Separator orientation="vertical" className="h-4" />
      <span className="text-sm font-medium">FitTrack</span>
    </header>
  );
}

export default Header;
