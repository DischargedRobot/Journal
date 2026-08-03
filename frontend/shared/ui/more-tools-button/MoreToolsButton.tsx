"use client"

import MoreVertIcon from "@mui/icons-material/MoreVert"
import IconButton from "@mui/material/IconButton"
import type { IconButtonProps } from "@mui/material/IconButton"
import Menu from "@mui/material/Menu"
import MenuItem from "@mui/material/MenuItem"
import { SxProps, Theme } from "@mui/material/styles"
import { memo, useState } from "react"

export type TMenuItemConfig = {
	key: string
	label: React.ReactNode
	onClick: () => void
	disabled?: boolean
	sx?: SxProps<Theme>
}

interface Props extends Omit<IconButtonProps, "onClick"> {
	items: TMenuItemConfig[]
	onMenuClick?: () => void
}

const MoreToolsButton = ({ onMenuClick, items, sx, ...props }: Props) => {
	const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null)

	const closeMenu = () => setAnchorEl(null)

	return (
		<>
			<IconButton
				size="small"
				disableRipple
				sx={[
					{
						fontSize: "clamp(16px, 12.80px + 1.00vw, 32px)",
						"&:hover": {
							color: "primary.main",
						},
					},
					...(Array.isArray(sx) ? sx : [sx]),
				]}
				onClick={(event) => {
					event.stopPropagation()
					setAnchorEl(event.currentTarget)
					onMenuClick?.()
				}}
				aria-label="Меню"
				aria-haspopup="true"
				aria-expanded={Boolean(anchorEl)}
				{...props}
			>
				<MoreVertIcon fontSize="inherit" />
			</IconButton>
			<Menu
				slotProps={{
					paper: { sx: { borderRadius: "20px" } },
				}}
				anchorEl={anchorEl}
				open={Boolean(anchorEl)}
				onClose={closeMenu}
			>
				{items.map((item) => (
					<MenuItem
						key={item.key}
						disabled={item.disabled}
						onClick={(event) => {
							event.stopPropagation()
							closeMenu()
							item.onClick()
						}}
						sx={item.sx}
					>
						{item.label}
					</MenuItem>
				))}
			</Menu>
		</>
	)
}

export default memo(MoreToolsButton)
