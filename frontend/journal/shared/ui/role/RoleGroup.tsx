import { TRole } from "@/shared/model/role"
import { AddChip } from "@/shared/ui/add-chip"
import Box from "@mui/material/Box"
import Chip from "@mui/material/Chip"
import ClearIcon from "@mui/icons-material/Clear"
interface Props {
    roles: TRole[]
    onAddRole?: (e: React.MouseEvent<HTMLDivElement>, roles: TRole[]) => void
    onClickRole?: (e: React.MouseEvent<HTMLDivElement>, role: TRole) => void
}

const RoleGroup = (props: Props) => {

    const {
        roles,
        onAddRole,
        onClickRole,
    } = props

    return (
        <Box className="flex items-center gap-1 rounded-[20px] "
            sx={{
                // backgroundColor: "secondary.dark",

            }}>
            <AddChip
                label="Добавить роль"
                onClick={(e) => {
                    onAddRole?.(e, roles)
                }}
            />
            {roles.map((role) => (
                <Chip
                    key={role.uuid}
                    label={role.name}
                    onDelete={(e) => {
                        onClickRole?.(e, role)

                    }}
                    deleteIcon={<ClearIcon />}
                    clickable
                    className="border-2"
                    sx={{
                        borderColor: "secondary.dark",
                        backgroundColor: "secondary.light",
                        "& .MuiChip-label": {
                            position: "relative",
                            left: "50%",
                            transform: "translateX(-50%)",
                            transition: "all 0.3s ease-out",
                        },

                        "& .MuiChip-deleteIcon ": {
                            position: "relative",
                            right: "30%",
                            margin: "0",
                            opacity: 0,
                            transform: "translateX(50%)",
                            transitionProperty: "right, opacity, transform, rotate",
                            transitionDuration: "0/3s",
                            transitionTimingFunction: "ease-out",
                        },
                        ":hover": {
                            backgroundColor: "warning.main",
                            color: "warning.contrastText",

                            "& .MuiChip-deleteIcon": {
                                position: "relative",
                                color: "warning.contrastText",
                                opacity: 1,
                                transform: "translateX(-50%)",
                                right: "0",
                            },

                            "& .MuiChip-label": {
                                transform: "translateX(0%)",
                                left: "0%",
                            }
                        },
                    }} />
            ))}
        </Box >

    )
}

export default RoleGroup