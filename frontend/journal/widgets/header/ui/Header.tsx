"use client"

import Link from "@mui/material/Link"
import AppBar from "@mui/material/AppBar"
import NextLink from "next/link"
import Toolbar from "@mui/material/Toolbar"
import Box from "@mui/material/Box"
import { Logo } from "@/shared/ui/Logo"
import Avatar from "@mui/material/Avatar"
import { ThemeIcon } from "@/shared/ui/theme-icon"

const Header = () => {
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
						color="default.main"
						underline="none"
						sx={{
							"&:hover": { color: "primary.main" },
						}}
					>
						Журнал
					</Link>
					<Link
						component={NextLink}
						href="/schedule"
						color="default.main"
						underline="none"
						sx={{
							"&:hover": { color: "primary.main" },
						}}
					>
						Расписание
					</Link>
					<Link
						component={NextLink}
						href="/"
						color="default.main"
						underline="none"
						sx={{
							"&:hover": { color: "primary.main" },
						}}
					>
						Сайт
					</Link>
				</Box>

				<Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
					<ThemeIcon />
					<Avatar />
				</Box>
			</Toolbar>
		</AppBar>
	)
}

export default Header
