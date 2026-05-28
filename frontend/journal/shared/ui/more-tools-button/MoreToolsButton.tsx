import IconButton from "@mui/material/IconButton"
import MoreVertIcon from "@mui/icons-material/MoreVert"
import type { IconButtonProps } from "@mui/material/IconButton"
import Menu from "@mui/material/Menu"
import { memo, useState } from "react"
import MenuItem from "@mui/material/MenuItem"

export interface MenuItemConfig {
    key: string
    label: React.ReactNode
    onClick: () => void
    disabled?: boolean
}

interface Props extends Omit<IconButtonProps, "onClick"> {
    onMenuClick: () => void
    items: MenuItemConfig[]
}

const MoreToolsButton = ({ onMenuClick, items, ...props }: Props) => {
    const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null)

    return (
        <>
            <IconButton
                size="small"
                onClick={(event) => {
                    event.stopPropagation()
                    setAnchorEl(event.currentTarget)
                    onMenuClick()
                }}
                aria-label="Меню "
                {...props}
            >
                <MoreVertIcon fontSize="small" />
            </IconButton>
            <Menu
                anchorEl={anchorEl}
                onClose={() => setAnchorEl(null)}
                open={!!anchorEl}
            >
                {items.map((item) => (
                    <MenuItem key={item.key} onClick={item.onClick} disabled={item.disabled} sx={{ color: "text.primary" }}>
                        {item.label}
                    </MenuItem>
                ))}
            </Menu>
        </>
    )
}

export default memo(MoreToolsButton)