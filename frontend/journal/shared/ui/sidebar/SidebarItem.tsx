import Divider from "@mui/material/Divider"
import ListItem from "@mui/material/ListItem"
import ListItemIcon from "@mui/material/ListItemIcon"
import ListItemText from "@mui/material/ListItemText"
import { Fragment } from "react/jsx-runtime"
import { useState } from "react"
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';

export interface Item {
    text: string
    href?: string
    icon?: React.ReactNode
    onClick?: () => void
    items?: Omit<Item, "items">[]
}

interface Props {
    item: Item
    isSelected: boolean
    onSelect: (item: Item) => void
}

const SideBarItem = ({ item, isSelected, onSelect }: Props) => {

    const hasItems = item.items && item.items.length > 0

    const [isOpen, setIsOpen] = useState(false)

    return (
        <Fragment>
            <ListItem
                className="cursor-pointer"
                onClick={() => {
                    if (hasItems) {
                        setIsOpen(!isOpen)
                    }
                    onSelect(item)
                    item.onClick?.()
                }}
                sx={isSelected ? {
                    borderLeftWidth: 5,
                    borderColor: "primary.main",
                } : {
                    opacity: 0.5,
                }}
            >
                {item.icon && <ListItemIcon>{item.icon}</ListItemIcon>}
                <ListItemText slotProps={{
                    primary: {
                        className: "title ",
                    },
                }}>
                    {item.text}
                </ListItemText>
                {hasItems && <ListItemIcon>
                    <ExpandMoreIcon className="transition-transform duration-300" fontSize="large" sx={isOpen ? { transform: "rotate(180deg)" } : {}} />
                </ListItemIcon>}
            </ListItem>
            {hasItems && isOpen && item.items!.map((item) => (
                <Fragment key={item.href ?? item.text}>
                    <ListItem>
                        <ListItemText>{item.text}</ListItemText>
                    </ListItem>
                </Fragment>
            ))}
            <Divider
                sx={{
                    borderColor: "grey.400",
                }}
            />
        </Fragment>
    )
}

export default SideBarItem
