import { NavLink } from "react-router-dom";
import {
  LayoutDashboard,
  Dumbbell,
  List,
  History,
  BarChart3,
  Settings,
} from "lucide-react";
import {
  Sidebar,
  SidebarHeader,
  SidebarContent,
  SidebarGroup,
  SidebarGroupLabel,
  SidebarGroupContent,
  SidebarMenu,
  SidebarMenuItem,
  SidebarMenuButton,
  useSidebar,
} from "@/components/ui/sidebar";

const navItems = [
  { to: "/", label: "Dashboard", icon: LayoutDashboard },
  { to: "/programs", label: "Programs", icon: Dumbbell },
  { to: "/exercises", label: "Exercises", icon: List },
  { to: "/history", label: "History", icon: History },
  { to: "/progress", label: "Analytics", icon: BarChart3 },
  { to: "/settings", label: "Settings", icon: Settings },
];

function AppSidebar() {
  const { setOpen, isMobile, setOpenMobile } = useSidebar();

  const handleMouseEnter = () => {
    if (!isMobile) setOpen(true);
  };
  const handleMouseLeave = () => {
    if (!isMobile) setOpen(false);
  };

  return (
    <Sidebar
      collapsible="icon"
      onMouseEnter={handleMouseEnter}
      onMouseLeave={handleMouseLeave}
    >
      <SidebarHeader>
        <div className="flex items-center gap-2 px-2 py-1 group-data-[collapsible=icon]:px-0">
          <Dumbbell className="size-6 shrink-0 text-primary" />
          <span className="text-lg font-semibold tracking-tight group-data-[collapsible=icon]:hidden">
            FitTrack
          </span>
        </div>
      </SidebarHeader>

      <SidebarContent>
        <SidebarGroup>
          <SidebarGroupLabel>Navigation</SidebarGroupLabel>
          <SidebarGroupContent>
            <SidebarMenu>
              {navItems.map((item) => (
                <SidebarMenuItem key={item.to}>
                  <SidebarMenuButton asChild tooltip={item.label}>
                    <NavLink
                      to={item.to}
                      end={item.to === "/"}
                      onClick={() => {
                        // close the drawer after navigating on mobile
                        if (isMobile) setOpenMobile(false);
                      }}
                      className={({ isActive }) =>
                        isActive
                          ? "bg-sidebar-accent text-sidebar-accent-foreground font-medium"
                          : ""
                      }
                    >
                      <item.icon className="size-4 shrink-0" />
                      <span>{item.label}</span>
                    </NavLink>
                  </SidebarMenuButton>
                </SidebarMenuItem>
              ))}
            </SidebarMenu>
          </SidebarGroupContent>
        </SidebarGroup>
      </SidebarContent>
    </Sidebar>
  );
}

export default AppSidebar;
