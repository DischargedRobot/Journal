import Checkbox from "@/shared/ui/checkbox/Checkbox"
import Box from "@mui/material/Box"
import ClickAwayListener from "@mui/material/ClickAwayListener"
import Popper from "@mui/material/Popper"
import Typography from "@mui/material/Typography"
import { TPresencesStatus } from "@/shared/model/presences-status"

interface Props {
    isOpen: boolean
    onClose: () => void
    anchorEl: HTMLElement | null
    onChange: (event: React.ChangeEvent<HTMLInputElement>, status: TPresencesStatus) => void
    selectedStatus?: TPresencesStatus
}

const SelectPresencesStatus = (props: Props) => {

    const { isOpen, onClose, anchorEl, onChange, selectedStatus } = props
    // проверяем что anchorEl существует и связан с DOM
    const validAnchorEl = anchorEl && anchorEl.isConnected
        ? anchorEl
        : null

    return (
        <Popper
            open={isOpen}
            anchorEl={validAnchorEl}
            placement="bottom-start"
            className="z-30"
        >
            <ClickAwayListener onClickAway={onClose ?? (() => { })}>
                <Box className="flex flex-col gap-2 p-3 rounded-[20px]"
                    sx={{
                        backgroundColor: "secondary.light",
                        boxShadow: "var(--shadow)",
                    }}>
                    <Box className="flex gap-1">
                        <Typography>О</Typography>
                        <Checkbox checked={selectedStatus === "О"} onChange={(event) => onChange(event, "О")} />
                    </Box>
                    <Box className="flex gap-1">
                        <Typography>Б</Typography>
                        <Checkbox checked={selectedStatus === "Б"} onChange={(event) => onChange(event, "Б")} />
                    </Box>
                    <Box className="flex gap-1">
                        <Typography>Н</Typography>
                        <Checkbox checked={selectedStatus === "Н"} onChange={(event) => onChange(event, "Н")} />
                    </Box>
                </Box>
            </ClickAwayListener>
        </Popper>
    )
}

export default SelectPresencesStatus