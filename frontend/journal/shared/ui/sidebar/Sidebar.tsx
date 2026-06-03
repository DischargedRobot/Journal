import List from "@mui/material/List"
import ListItem from "@mui/material/ListItem"
import ListItemText from "@mui/material/ListItemText"
import { useState } from "react"
import SideBarItem, { type Item } from "./SidebarItem"

interface Props {
    open?: boolean
    children?: React.ReactNode
    onClose?: () => void
    title?: string
    items?: Item[]
    className?: string
}
const Sidebar = (props: Props) => {

    const {
        open = true,
        onClose,
        title,
        items,
        className,
    } = props

    const [selectedItem, setSelectedItem] = useState<Item | null>(null)


    return (
        <List
            className={`flex flex-col p-0 w-full max-w-[240px] h-full shadow-2xl ${className}`}
            sx={{
                backgroundColor: "secondary.light",
            }}
        >
            {title && title.length > 0 && <ListItem
                className="flex items-center justify-center py-4 text-center"
                sx={{
                    backgroundColor: "primary.main",
                    color: "secondary.light",
                }}>
                <ListItemText
                    slotProps={{
                        primary: {
                            className: "title title_large",
                        },
                    }}
                >
                    {title}
                </ListItemText>
            </ListItem>}
            {items?.map((item) => (
                <SideBarItem
                    key={item.href ?? item.text}
                    item={item}
                    isSelected={selectedItem === item}
                    onSelect={setSelectedItem}
                />
            ))}
        </List>

    )
}

export default Sidebar