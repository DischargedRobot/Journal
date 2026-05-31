import { TRole } from "@/shared/model/role"
import Box from "@mui/material/Box"
import Chip from "@mui/material/Chip"
import AddIcon from '@mui/icons-material/Add';

interface Props {
    roles: TRole[]
}

const RoleGroup = ({ roles }: Props) => {
    return (
        <Box className="flex items-center gap-1 rounded-[20px] p-1 border"
            sx={{
                backgroundColor: "secondary.light",
                borderColor: "secondary.dark",

            }}>
            <Chip
                clickable
                icon={<AddIcon />}
                label="Добавить роль"
                onClick={() => {
                    console.log("Добавить роль")
                }}
                sx={{
                    backgroundColor: "secondary.light",
                    ":hover": {
                        backgroundColor: "secondary.main",
                    },

                    "& .MuiTouchRipple-root": {
                        color: "primary.main",
                    },

                }}
            />
            {roles.map((role) => (
                <Chip key={role.uuid} label={role.name} sx={{
                    backgroundColor: "secondary.light",
                }} />
            ))}
        </Box >

    )
}

export default RoleGroup