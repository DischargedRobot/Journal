import Divider from "@mui/material/Divider"
import List from "@mui/material/List"
import ListItem from "@mui/material/ListItem"
import ListItemIcon from "@mui/material/ListItemIcon"
import ListItemText from "@mui/material/ListItemText"
import { Fragment, useState } from "react"



interface Item {
    text: string
    href?: string
    icon?: React.ReactNode
    onClick?: () => void
}

interface Props {
    open?: boolean
    children?: React.ReactNode
    onClose?: () => void
    title?: string
    items?: Item[]
}
const Sidebar = (props: Props) => {

    const {
        open = true,
        onClose,
        title,
        items,
    } = props

    const [selectedItem, setSelectedItem] = useState<Item | null>(null)

    return (
        <List
            className="flex flex-col p-0 w-full max-w-[330px] h-full shadow-2xl"
            sx={{
                backgroundColor: "secondary.light",
            }}
        >
            <ListItem
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
            </ListItem>
            {items?.map((item) => (
                <Fragment key={item.href ?? item.text}>
                    <ListItem
                        className=" cursor-pointer"
                        onClick={() => {
                            setSelectedItem(item)
                            item.onClick?.()
                        }}
                        sx={selectedItem?.href === item.href ? {
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
                    </ListItem>
                    <Divider
                        sx={{
                            borderColor: "grey.400",
                        }}
                    />
                </Fragment>
            ))}
        </List>

    )
}

export default Sidebar