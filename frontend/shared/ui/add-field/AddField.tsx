import { SxProps } from "@mui/material"
import Button from "@mui/material/Button"
import Typography from "@mui/material/Typography"
import AddIcon from "@mui/icons-material/Add"

interface Props {
    label: string
    onClick: (e: React.MouseEvent<HTMLButtonElement>) => void
    className?: string
    sx?: SxProps
}

const AddField = (props: Props) => {
    const {
        label = "Добавить",
        onClick,
        className,
        sx,
    } = props
    return (
        <Button
            onClick={onClick}
            startIcon={<AddIcon sx={{ width: 24, height: 24 }} />}
            className={`flex items-center gap-2 ${className}`}
            sx={
                [
                    {
                        backgroundColor: "secondary.light",
                        color: "contrastingSecondary.main",
                        "&:hover": {
                            backgroundColor: "primary.main",
                            color: "primary.contrastText",
                        },
                    }, ...(Array.isArray(sx) ? sx : [sx])
                ]}>
            <Typography variant="body1">{label}</Typography>
        </Button>
    )
}

export default AddField