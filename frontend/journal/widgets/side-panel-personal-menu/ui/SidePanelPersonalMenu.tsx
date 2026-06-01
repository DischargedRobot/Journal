"use client"

import List from "@mui/material/List"
import ListItem from "@mui/material/ListItem"
import ListItemIcon from "@mui/material/ListItemIcon"
import ListItemText from "@mui/material/ListItemText"
import ArrowBack from "@mui/icons-material/ArrowBack"
import { StudentsIcon } from "@/shared/ui/students-icon"
import { DepartmentsIcon } from "@/shared/ui/departments-icon"
import BookOpenIcon from "@/shared/ui/book-open-icon/BookOpenIcon"
import { TeacherIcon } from "@/shared/ui/teacher-icon"
import { usePathname } from "next/navigation"
import "./SidePanelPersonalMenu.css"
import NextLink from "next/link"


const menuItems = [

    {
        icon: <BookOpenIcon fontSize="small" />,
        text: "ПАРЫ",
        href: "/personal/my-lessons",
    },
    {
        icon: <StudentsIcon fontSize="small" />,
        text: "СТУДЕНТЫ",
        href: "/personal/students",
    },
    {
        icon: <DepartmentsIcon fontSize="small" />,
        text: "КАФЕДРЫ",
        href: "/personal/departments",
    },
    {
        icon: <ArrowBack fontSize="small" />,
        text: "ДИСЦИПЛИНЫ",
        href: "/personal/disciplines",
    },
    {
        icon: <TeacherIcon fontSize="small" />,
        text: "ПРЕПОДАВАТЕЛИ",
        href: "/personal/professors",
    },

]



const SidePanelPersonalMenu = () => {

    const pathname = usePathname()
    const activeItem = (href: string) => {
        return pathname.startsWith(href)
    }

    return (
        <List
            component="menu"
            className="flex self-stretch flex-col items-center justify-start gap-2 p-2 w-fit h-full shadow-2xl z-10"
            sx={{
                backgroundColor: "secondary.main",
            }}>
            {menuItems.map((item) => (
                <ListItem
                    className={`side-panel-personal-menu__item flex flex-col min-h-[95px] items-center justify-center py-0 w-full rounded-[20px] cursor-pointer ${activeItem(item.href) ? "side-panel-personal-menu__item_active" : ""}`}
                    key={item.href}
                    component={NextLink}
                    href={item.href}
                    sx={{
                        backgroundColor: activeItem(item.href)
                            ? "primary.main"
                            : "transparent",
                        color: activeItem(item.href)
                            ? "secondary.light"
                            : "contrastingSecondary.main",
                        // "&:hover": {
                        //     backgroundColor: "primary.main",
                        //     color: "secondary.light",
                        // },
                    }}
                >
                    <ListItemIcon>
                        {item.icon}
                    </ListItemIcon>
                    <ListItemText className="flex-0!" slotProps={{
                        primary: {
                            className: "title title_x-litle font-bold"
                        }
                    }}>
                        {item.text}
                    </ListItemText>
                </ListItem>
            ))}
        </List>
    )
}

export default SidePanelPersonalMenu
