"use client"

import Link from "@mui/material/Link"
import AppBar from "@mui/material/AppBar"
import NextLink from "next/link"
import { usePathname } from "next/navigation"
import Toolbar from "@mui/material/Toolbar"
import Box from "@mui/material/Box"
import { Logo } from "@/shared/ui/Logo"
import { ProfileAvatarMenu } from "@/features/profile-avatar-menu"
import { ThemeIcon } from "@/shared/ui/theme-icon"

const Header = () => {
	const pathname = usePathname() || ""

	const isActive = (href: string) => {
		if (href === "/") return pathname === "/"
		return pathname === href || pathname.startsWith(href + "/")
	}

	return (
		<AppBar
			position="sticky"
			sx={{
				backgroundColor: "secondary.main",
			}}
		>
			<Toolbar
				component="nav"
				aria-label="main navigation"
				sx={{
					display: "flex",
					alignItems: "center",
					justifyContent: "space-between",
					gap: 2,
					marginY: 1,
				}}
			>
				<Box sx={{ display: "flex", alignItems: "center", gap: 2 }}>
					<Logo />
				</Box>

				<Box
					className="title"
					sx={{
						display: "flex",
						alignItems: "center",
						gap: 2,
						flexGrow: 1,
						justifyContent: "center",
					}}
				>
					<Link
						component={NextLink}
						href="/journal"
						underline="none"
						sx={{
							color: isActive("/journal")
								? "primary.main"
								: "default.main",
							"&:hover": { color: "primary.light" },
						}}
					>
						Журнал
					</Link>
					<Link
						component={NextLink}
						href="/schedule"
						underline="none"
						sx={{
							color: isActive("/schedule")
								? "primary.main"
								: "default.main",
							"&:hover": { color: "primary.light" },
						}}
					>
						Расписание
					</Link>
					<Link
						component={NextLink}
						href="/"
						underline="none"
						sx={{
							color: isActive("/")
								? "primary.main"
								: "default.main",
							"&:hover": { color: "primary.main" },
						}}
					>
						Сайт
					</Link>
				</Box>

				<Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
					<ThemeIcon />
					<ProfileAvatarMenu />
				</Box>
			</Toolbar>
		</AppBar>
	)
}

export default Header
