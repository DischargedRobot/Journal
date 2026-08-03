import { CheckboxRect } from "@/shared/ui/checkbox"
import Box from "@mui/material/Box"
import ClickAwayListener from "@mui/material/ClickAwayListener"
import Popper from "@mui/material/Popper"
import Typography from "@mui/material/Typography"
import { TPresencesStatus } from "@/shared/model/presences-status"
import { useCallback, useLayoutEffect, useRef, useState } from "react"
import SelectAbsenceStatus from "./SelectAbsenceStatus"

interface Props {
    isOpen: boolean
    onClose: () => void
    anchorEl: HTMLElement | null
    onChange: (event: React.ChangeEvent<HTMLInputElement>, status: TPresencesStatus) => void
    selectedStatus?: TPresencesStatus
    absenceStatusDenominator?: number
}
/** Вместе с Н*/
const isFullAbsenceStatus = (status: TPresencesStatus | undefined) =>
    status === "Н" || isAbsenceStatus(status)

/** Только частичное отсутствие, например 1/2 */
const isAbsenceStatus = (status: TPresencesStatus | undefined) =>
    /^\d+\/\d+$/.test(status ?? "")

const createPresenceChangeEvent = () =>
    ({ stopPropagation: () => { } }) as React.ChangeEvent<HTMLInputElement>

interface PanelProps {
    onClose: () => void
    onChange: (event: React.ChangeEvent<HTMLInputElement>, status: TPresencesStatus) => void
    selectedStatus?: TPresencesStatus
    absenceStatusDenominator: number
}

const SelectPresencesStatusPanel = ({
    onClose,
    onChange,
    selectedStatus,
    absenceStatusDenominator,
}: PanelProps) => {
    const absenceRowRef = useRef<HTMLDivElement>(null)
    const hasSelectedAbsenceStatusRef = useRef(false)
    const [isAbsenceStatusOpen, setIsAbsenceStatusOpen] = useState(() =>
        isAbsenceStatus(selectedStatus),
    )
    const [absenceStatusAnchorEl, setAbsenceStatusAnchorEl] = useState<HTMLElement | null>(null)

    const applyDefaultAbsence = useCallback(() => {
        if (hasSelectedAbsenceStatusRef.current || isAbsenceStatus(selectedStatus)) {
            return
        }
        onChange(createPresenceChangeEvent(), "Н")
    }, [onChange, selectedStatus])

    // чтобы открывался без возможной задержки 
    useLayoutEffect(() => {
        if (!isAbsenceStatusOpen || absenceStatusAnchorEl) {
            return
        }
        const anchor = absenceRowRef.current
        if (anchor) {
            setAbsenceStatusAnchorEl(anchor)
        }
    }, [absenceStatusAnchorEl, isAbsenceStatusOpen])

    const handleClose = useCallback(() => {
        if (isAbsenceStatusOpen) {
            applyDefaultAbsence()
        }
        setIsAbsenceStatusOpen(false)
        onClose()
    }, [applyDefaultAbsence, isAbsenceStatusOpen, onClose])

    const openAbsenceStatus = useCallback(() => {
        const anchor = absenceRowRef.current
        if (!anchor) {
            return
        }
        hasSelectedAbsenceStatusRef.current = false
        setAbsenceStatusAnchorEl(anchor)
        setIsAbsenceStatusOpen(true)
    }, [])

    const handleClickAway = useCallback(
        (event: MouseEvent | TouchEvent) => {
            const target = event.target
            if (target instanceof Element && target.closest("[data-absence-status-popper]")) {
                return
            }
            handleClose()
        },
        [handleClose],
    )

    const handleChange = useCallback(
        (event: React.ChangeEvent<HTMLInputElement>, status: TPresencesStatus) => {
            if (isAbsenceStatus(status)) {
                hasSelectedAbsenceStatusRef.current = true
            }
            setIsAbsenceStatusOpen(false)
            onChange(event, status)
        },
        [onChange],
    )

    return (
        <ClickAwayListener onClickAway={handleClickAway}>
            <Box
                className="flex flex-col gap-2 p-3 rounded-[20px]"
                sx={{
                    backgroundColor: "secondary.light",
                    boxShadow: "var(--shadow)",
                    overflow: "visible",
                }}
            >
                <Box className="flex items-center justify-between gap-1">
                    <Typography>О</Typography>
                    <CheckboxRect
                        checked={selectedStatus === "О" && !isAbsenceStatusOpen}
                        onChange={(event) => handleChange(event, "О")}
                    />
                </Box>
                <Box className="flex items-center justify-between gap-1">
                    <Typography>Б</Typography>
                    <CheckboxRect
                        checked={selectedStatus === "Б" && !isAbsenceStatusOpen}
                        onChange={(event) => handleChange(event, "Б")}
                    />
                </Box>
                <Box
                    ref={absenceRowRef}
                    className="flex items-center justify-between gap-1 cursor-pointer"
                    onClick={openAbsenceStatus}
                >
                    <Typography>Н</Typography>
                    <CheckboxRect
                        checked={isFullAbsenceStatus(selectedStatus) || isAbsenceStatusOpen}
                        onClick={(event) => {
                            event.stopPropagation()
                            openAbsenceStatus()
                        }}
                    />
                </Box>
                <SelectAbsenceStatus
                    isOpen={isAbsenceStatusOpen}
                    anchorEl={absenceStatusAnchorEl}
                    absenceStatusDenominator={absenceStatusDenominator}
                    selectedStatus={selectedStatus}
                    onChange={handleChange}
                />
            </Box>
        </ClickAwayListener>
    )
}

const SelectPresencesStatus = (props: Props) => {
    const {
        isOpen,
        onClose,
        anchorEl,
        onChange,
        selectedStatus,
        absenceStatusDenominator = 2,
    } = props

    const validAnchorEl = anchorEl && anchorEl.isConnected
        ? anchorEl
        : null

    return (
        <Popper
            open={isOpen}
            anchorEl={validAnchorEl}
            placement="bottom-start"
            className="z-100"
        >
            {isOpen ? (
                <SelectPresencesStatusPanel
                    onClose={onClose}
                    onChange={onChange}
                    selectedStatus={selectedStatus}
                    absenceStatusDenominator={absenceStatusDenominator}
                />
            ) : null}
        </Popper>
    )
}

export default SelectPresencesStatus
