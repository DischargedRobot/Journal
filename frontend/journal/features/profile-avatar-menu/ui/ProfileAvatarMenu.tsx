"use client"

import { memo, useState } from "react"
import Avatar from "@mui/material/Avatar"
import IconButton from "@mui/material/IconButton"
import Menu from "@mui/material/Menu"
import MenuItem from "@mui/material/MenuItem"
import ListItemIcon from "@mui/material/ListItemIcon"
import ListItemText from "@mui/material/ListItemText"
import PersonIcon from "@mui/icons-material/Person"
import Logout from "@mui/icons-material/Logout"
import { fluidClamp } from "@/shared/lib/fluidClampPx"
import NextLink from "next/link"



interface Props {
	minIconSize?: number
	maxIconSize?: number
}

const ProfileAvatarMenu = ({ minIconSize = 24, maxIconSize = 64 }: Props) => {
	const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null)
	const open = Boolean(anchorEl)

	const openMenu = (event: React.MouseEvent<HTMLElement>) => {
		setAnchorEl(event.currentTarget)
	}

	const closeMenu = () => setAnchorEl(null)
	const avatarSize = fluidClamp(minIconSize, maxIconSize)



	return (
		<>
			<IconButton
				onClick={openMenu}
				aria-label="Меню профиля"
				aria-controls={open ? "profile-avatar-menu" : undefined}
				aria-haspopup="true"
				aria-expanded={open ? "true" : undefined}
			>
				<Avatar sx={{ width: avatarSize, height: avatarSize }} />
			</IconButton>
			<Menu
				id="profile-avatar-menu"
				anchorEl={anchorEl}
				open={open}
				onClose={closeMenu}
				onClick={closeMenu}
				transformOrigin={{ vertical: "top", horizontal: "center" }}
			>
				<MenuItem >
					<NextLink
						className="flex "
						href="/personal/my-lessons"
					>
						<ListItemIcon>
							<PersonIcon fontSize="small" />
						</ListItemIcon>
						<ListItemText>Профиль</ListItemText>
					</NextLink>
				</MenuItem>
				<MenuItem >
					<NextLink
						className="flex "
						href="/personal/my-lessons"
					>
						<ListItemIcon>
							<PersonIcon fontSize="small" />
						</ListItemIcon>
						<ListItemText>Мои занятия</ListItemText>
					</NextLink>
				</MenuItem>
				<MenuItem>
					<ListItemIcon>
						<Logout fontSize="small" />
					</ListItemIcon>
					<ListItemText>Выйти</ListItemText>
				</MenuItem>
			</Menu>
		</>
	)
}

export default memo(ProfileAvatarMenu)
