import Box from "@mui/material/Box"
import Stack from "@mui/material/Stack"
import {
	Children,
	cloneElement,
	isValidElement,
	ReactElement,
	ReactNode,
} from "react"
import { useWizard } from "./Wizard"
import { type ClassValue } from "clsx"
import { classNamesTwMerge } from "@/shared/lib/classNamesTwMerge"

type StepChildProps = {
	stepId?: number | string
	children?: ReactNode
}

export const Step = ({
	children,
	stepId,
}: {
	children?: ReactNode
	stepId?: number | string
}) => {
	return (
		<>
			{Children.map(children, (child) => {
				if (!isValidElement(child)) {
					return child
				}

				if (child.type === StepHeader || child.type === StepContent) {
					return cloneElement(child as ReactElement<StepChildProps>, {
						stepId,
					})
				}

				return child
			})}
		</>
	)
}

export const Label = ({
	className,
	children,
}: {
	className?: string
	children: ReactNode
}) => {
	return <span className={className}>{children}</span>
}

export const StepContent = ({
	children,
	stepId,
}: {
	children: ReactNode
	stepId?: number | string
}) => {
	const { currentStep } = useWizard()

	return (
		<Stack
			className="flex-1"
			sx={{
				gridArea: "1 / 1",
				visibility: stepId === currentStep ? "visible" : "hidden",
				pointerEvents: stepId === currentStep ? "auto" : "none",
			}}
		>
			{children}
		</Stack>
	)
}

interface Props {
	children: ReactNode
	stepId?: number | string
	errorMessage?: string | null
	classNames?: {
		label?: ClassValue
		errorMessage?: ClassValue
		number?: ClassValue
	}
}

export const StepHeader = (props: Props) => {
	const { children, stepId, errorMessage, classNames } = props
	const { currentStep, setCurrentStep } = useWizard()

	return (
		<Box
			className="cursor-pointer grid grid-cols-[auto_1fr] grid-rows-[auto_1fr] gap-x-2 items-center"
			onClick={() => {
				if (stepId !== undefined) {
					setCurrentStep(stepId)
				}
			}}
		>
			<Box
				className={
					classNames?.number
						? classNamesTwMerge(
								"col-start-1 row-start-1 flex items-center justify-center rounded-full border-4",
								classNames?.number,
							)
						: "col-start-1 row-start-1 flex items-center justify-center rounded-full border-4"
				}
				sx={{
					width: "2.5rem",
					height: "2.5rem",
					backgroundColor:
						stepId === currentStep
							? "primary.main"
							: "secondary.main",
					borderColor: "primary.main",
					color:
						stepId === currentStep
							? "primary.contrastText"
							: "secondary.contrastText",
				}}
			>
				{stepId}
			</Box>
			<Label
				className={
					classNames?.label
						? classNamesTwMerge(
								"col-start-2 row-start-1 title title_small",
								classNames?.label,
							)
						: "col-start-2 row-start-1 title title_small"
				}
			>
				{children}
			</Label>
			<Box
				className={
					classNames?.errorMessage
						? classNamesTwMerge(
								"col-start-2 row-start-2",
								classNames?.errorMessage,
							)
						: "col-start-2 row-start-2"
				}
				sx={{
					color: "error.main",
					fontSize: "0.8rem",
					visibility: errorMessage ? "visible" : "hidden",
				}}
			>
				{errorMessage ?? "&ZeroWidthSpace"}
			</Box>
		</Box>
	)
}
