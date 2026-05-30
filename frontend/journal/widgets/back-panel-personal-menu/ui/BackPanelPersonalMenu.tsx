import List from "@mui/material/List"
import ListItem from "@mui/material/ListItem"
import ListItemIcon from "@mui/material/ListItemIcon"
import ListItemText from "@mui/material/ListItemText"
import ArrowBack from "@mui/icons-material/ArrowBack"
import { StudentsIcon } from "@/shared/ui/students-icon"
import { DepartmentsIcon } from "@/shared/ui/departments-icon"

const menuItems = [

    {
        icon: <DepartmentsIcon fontSize="small" />,
        text: "Пары",
        href: "/personal/my-lessons",
    },
    {
        icon: <StudentsIcon fontSize="small" />,
        text: "Студенты",
        href: "/personal/students",
    },
    {
        icon: <ArrowBack fontSize="small" />,
        text: "Кафедры",
        href: "/personal/departments",
    },
    {
        icon: <ArrowBack fontSize="small" />,
        text: "Дисциплины",
        href: "/personal/my-disciplines",
    },
    {
        icon: <ArrowBack fontSize="small" />,
        text: "Преподаватели",
        href: "/personal/professors",
    },

]

const BackPanelPersonalMenu = () => {
    return (
        <List component="aside">
            {menuItems.map((item) => (
                <ListItem key={item.href}>
                    <ListItemIcon>
                        {item.icon}
                    </ListItemIcon>
                    <ListItemText>
                        {item.text}
                    </ListItemText>
                </ListItem>
            ))}
        </List>
    )
}

export default BackPanelPersonalMenu